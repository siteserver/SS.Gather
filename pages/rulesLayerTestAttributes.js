var $api = axios.create({
  baseURL: utils.getQueryString('apiUrl') + '/' + utils.getQueryString('pluginId') + '/pages/rulesLayerTestAttributes/',
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
  url: utils.getQueryString('url'),
  attributeList: null
};

var methods = {
  apiGet: function () {
    var $this = this;

    if ($this.pageLoad) utils.loading(true);
    $api.post('', {
      contentUrl: this.url
    }).then(function (response) {
      var res = response.data;

      $this.attributeList = res.value;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      $this.pageLoad = true;
      utils.loading(false);
    });
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
