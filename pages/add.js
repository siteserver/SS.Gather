var $api = axios.create({
  baseURL: utils.getQueryString('apiUrl') + '/' + utils.getQueryString('pluginId') + '/pages/add/',
  withCredentials: true
});

var $typeBase = 'Base';
var $typeList = 'List';
var $typeContent = 'Content';
var $typeAdvanced = 'Advanced';
var $typeDone = 'Done';

var data = {
  siteId: utils.getQueryInt('siteId'),
  ruleId: utils.getQueryInt('ruleId'),
  pageLoad: false,
  pageAlert: null,
  pageType: null,
  pageProgress: '',
  ruleInfo: null,
  contentHtmlClearList: null,
  contentHtmlClearTagList: null,
  channels: null,
  charsets: null,
};

var methods = {
  insert: function(ref, text) {
    var tArea = this.$refs[ref];
    var startPos = tArea.selectionStart;
    var endPos = tArea.selectionEnd;
    var tmpStr = this.ruleInfo[ref] || '';
    this.ruleInfo[ref] = tmpStr.substring(0, startPos) + text + tmpStr.substring(endPos, tmpStr.length);
  },

  setState: function(pageType) {
    this.pageType = pageType;
    if (this.pageType === $typeBase) {
      this.pageProgress =  ''
      this.pageAlert = {
        type: 'primary',
        html: '在此设置采集基本属性'
      }
    } else if (this.pageType === $typeList) {
      this.pageProgress = '25%';
      this.pageAlert = {
        type: 'primary',
        html: '在此设置需要采集的列表页面地址'
      }
    } else if (this.pageType === $typeContent) {
      this.pageProgress = '50%';
      this.pageAlert = {
        type: 'primary',
        html: '在此设置需要采集的内容页面规则'
      }
    } else if (this.pageType === $typeAdvanced) {
      this.pageProgress = '75%';
      this.pageAlert = {
        type: 'primary',
        html: '在此设置采集高级配置，没有特殊规则可以直接点击下一步'
      }
    } else if (this.pageType === $typeDone) {
      this.pageProgress = '100%';
      this.pageAlert = null;
    }
    utils.up();
  },

  apiGet: function () {
    var $this = this;

    $api.get('', {
      params: {
        siteId: $this.siteId,
        ruleId: $this.ruleId
      }
    }).then(function (response) {
      var res = response.data;

      $this.ruleInfo = res.value;
      $this.channels = res.channels;
      $this.charsets = res.charsets;
      $this.contentHtmlClearList = res.contentHtmlClearList;
      $this.contentHtmlClearTagList = res.contentHtmlClearTagList;

      $this.setState($typeBase);
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      $this.pageLoad = true;
    });
  },

  apiSubmit: function () {
    var $this = this;

    utils.loading(true);
    $api.post('?siteId=' + this.siteId, {
      ruleInfo: this.ruleInfo,
      contentHtmlClearList: this.contentHtmlClearList,
      contentHtmlClearTagList: this.contentHtmlClearTagList,
    }).then(function (response) {
      var res = response.data;

      $this.setState($typeDone);

      setTimeout(function() {
        location.href = utils.getPageUrl('rules.html');
      }, 1500);
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  },

  btnPreviousClick: function () {
    var pageType = '';
    if (this.pageType === $typeList) {
      pageType = $typeBase;
    } else if (this.pageType === $typeContent) {
      pageType = $typeList;
    } else if (this.pageType === $typeAdvanced) {
      pageType = $typeContent;
    } else if (this.pageType === $typeDone) {
      pageType = $typeAdvanced;
    }
    this.setState(pageType);
  },

  btnSubmitClick: function () {
    var $this = this;
    this.pageAlert = null;

    this.$validator.validateAll('ruleInfo').then(function (result) {
      if (result) {
        var pageType = '';
        if ($this.pageType === $typeBase) {
          pageType = $typeList;
        } else if ($this.pageType === $typeList) {
          if ($this.ruleInfo.gatherUrlIsSerialize && $this.ruleInfo.gatherUrlSerialize.indexOf('*') === -1) {
            swal2.error('序列相似网址必须包含 * 通配符');
            return;
          }
          pageType = $typeContent;
        } else if ($this.pageType === $typeContent) {
          pageType = $typeAdvanced;
        } else if ($this.pageType === $typeAdvanced) {
          $this.apiSubmit();
          return;
        }
        $this.setState(pageType);
      }
    });
  }
};

Vue.component("date-picker", window.DatePicker.default);

var $vue = new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.apiGet();
  }
});
