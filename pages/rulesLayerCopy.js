var $api = axios.create({
  baseURL: utils.getQueryString('apiUrl') + '/' + utils.getQueryString('pluginId') + '/pages/rulesLayerCopy/',
  withCredentials: true,
  params: {
    siteId: utils.getQueryInt('siteId')
  }
});

var data = {
  ruleId: utils.getQueryInt('ruleId'),
  pageLoad: false,
  pageAlert: null,
  ruleName: null
};

var methods = {
  apiCopy: function () {
    var $this = this;

    utils.loading(true);
    $api.post('', {
      ruleId: this.ruleId,
      ruleName: this.ruleName
    }).then(function (response) {
      var res = response.data;

      parent.location.reload(true);
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  },

  btnSubmitClick: function () {
    var $this = this;
    this.pageAlert = null;

    this.$validator.validate().then(function (result) {
      if (result) {
        $this.apiCopy();
      }
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
    this.pageLoad = true;
  }
});
