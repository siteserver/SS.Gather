var $api = axios.create({
  baseURL: utils.getQueryString('apiUrl') + '/' + utils.getQueryString('pluginId') + '/pages/rulesLayerTest/',
  withCredentials: true,
  params: {
    siteId: utils.getQueryInt('siteId'),
    ruleId: utils.getQueryInt('ruleId')
  }
});

var data = {
  pageLoad: false,
  pageAlert: null,
  ruleInfo: null,
  gatherUrls: null,
  listUrl: null,
  contentUrl: null,
  contentUrls: null,
  attributeList: null
};

var methods = {
  apiGet: function () {
    var $this = this;

    if ($this.pageLoad) utils.loading(true);
    $api.get('').then(function (response) {
      var res = response.data;

      $this.ruleInfo = res.value;
      $this.gatherUrls = res.gatherUrls;
      $this.listUrl = res.gatherUrls[0];
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      $this.pageLoad = true;
      utils.loading(false);
    });
  },

  apiGetContentUrls: function () {
    var $this = this;

    utils.loading(true);
    $api.post('actions/getContentUrls', {
      gatherUrl: this.listUrl
    }).then(function (response) {
      var res = response.data;

      $this.contentUrls = res.value;
      $this.pageAlert = {
        type: 'success',
        html: '采集名称：' + $this.ruleInfo.gatherRuleName + '  内容数：' + $this.contentUrls.length
      };
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  },

  apiGetContent: function (url) {
    this.contentUrl = url;
    var $this = this;

    utils.loading(true);
    $api.post('actions/getContent', {
      contentUrl: url
    }).then(function (response) {
      var res = response.data;

      $this.attributeList = res.value;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  },

  btnGetContentUrls: function () {
    this.pageAlert = null;
    this.apiGetContentUrls();
  },

  btnGetContent: function(url) {
    this.pageAlert = null;
    this.apiGetContent(url);
  },

  btnCancelClick: function () {
    utils.closeLayer();
  }
};

new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.apiGet();
  }
});
