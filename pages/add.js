var $api = axios.create({
  baseURL: utils.getQueryString('apiUrl') + '/' + utils.getQueryString('pluginId') + '/pages/add/',
  withCredentials: true
});

var $typeBase = 'Base';
var $typeList = 'List';
var $typeContent = 'Content';
var $typeColumns = 'Columns';
var $typeAdvanced = 'Advanced';
var $typeDone = 'Done';

var data = {
  siteId: utils.getQueryInt('siteId'),
  ruleId: utils.getQueryInt('ruleId'),
  pageLoad: false,
  pageAlert: null,
  pageType: null,
  ruleInfo: null,
  channels: [],
  charsets: [],
  contentHtmlClearList: [],
  contentHtmlClearTagList: [],
  attributes: [],
  attributesDict: {}
};

var methods = {
  insert: function(ref, text) {
    var tArea = document.getElementById(ref);
    var startPos = tArea.selectionStart;
    var endPos = tArea.selectionEnd;
    var tmpStr = this.ruleInfo[ref] || '';
    this.ruleInfo[ref] = tmpStr.substring(0, startPos) + text + tmpStr.substring(endPos, tmpStr.length);
  },

  insertDict: function(ref, text) {
    var tArea = document.getElementById(ref);
    var startPos = tArea.selectionStart;
    var endPos = tArea.selectionEnd;
    var tmpStr = this.attributesDict[ref] || '';
    this.attributesDict[ref] = tmpStr.substring(0, startPos) + text + tmpStr.substring(endPos, tmpStr.length);
  },

  setState: function(pageType) {
    this.pageType = pageType;
    if (this.pageType === $typeBase) {
      this.pageAlert = {
        type: 'success',
        html: '在此设置采集基本属性'
      }
    } else if (this.pageType === $typeList) {
      this.pageAlert = {
        type: 'success',
        html: '在此设置需要采集的列表页面地址'
      }
    } else if (this.pageType === $typeContent) {
      this.pageAlert = {
        type: 'success',
        html: '在此设置需要采集的内容页面标题及正文规则'
      }
    } else if (this.pageType === $typeColumns) {
      this.pageAlert = {
        type: 'success',
        html: '在此设置需要采集的内容页面可选字段规则'
      }
    } else if (this.pageType === $typeAdvanced) {
      this.pageAlert = {
        type: 'success',
        html: '在此设置采集高级选项，没有特殊规则可以直接点击下一步'
      }
    } else if (this.pageType === $typeDone) {
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
      $this.attributesDict = res.attributesDict;

      $this.setState($typeBase);
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      $this.pageLoad = true;
    });
  },

  apiGetAttributes: function () {
    var $this = this;

    utils.loading(true);
    $api.get('attributes', {
      params: {
        siteId: $this.siteId,
        channelId: $this.ruleInfo.channelId,
        ruleId: $this.ruleId
      }
    }).then(function (response) {
      var res = response.data;

      $this.attributes = res.value;

      $this.setState($typeColumns);
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  },

  apiSubmit: function () {
    var $this = this;

    utils.loading(true);
    
    var contentAttributeList = [];
    for (var i = 0; i < this.attributes.length; i++) {
      var attribute = this.attributes[i];
      if (attribute.selected) {
        contentAttributeList.push(attribute.value);
      }
    }
    var payload = {
      ruleInfo: this.ruleInfo,
      contentHtmlClearList: this.contentHtmlClearList,
      contentHtmlClearTagList: this.contentHtmlClearTagList,
      contentAttributeList: contentAttributeList,
      attributesDict: this.attributesDict
    };

    $api.post('?siteId=' + this.siteId, payload).then(function (response) {
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

  getStartName: function(attribute) {
    return (attribute.value + '_start').toLowerCase();
  },

  getEndName: function(attribute) {
    return (attribute.value + '_end').toLowerCase();
  },

  getDefaultName: function(attribute) {
    return (attribute.value + '_default').toLowerCase();
  },

  btnPreviousClick: function () {
    var pageType = '';
    if (this.pageType === $typeList) {
      pageType = $typeBase;
    } else if (this.pageType === $typeContent) {
      pageType = $typeList;
    } else if (this.pageType === $typeColumns) {
      pageType = $typeContent;
    } else if (this.pageType === $typeAdvanced) {
      pageType = $typeColumns;
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
          $this.apiGetAttributes();
          return;
        } else if ($this.pageType === $typeColumns) {
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
