var $api = axios.create({
  baseURL: utils.getQueryString('apiUrl') + '/' + utils.getQueryString('pluginId') + '/pages/rules/',
  withCredentials: true
});

var data = {
  siteId: utils.getQueryInt('siteId'),
  pageLoad: false,
  pageAlert: null,
  ruleInfoList: null
};

var methods = {
  apiGetList: function () {
    var $this = this;

    if ($this.pageLoad) utils.loading(true);
    $api.get('', {
      params: {
        siteId: $this.siteId
      }
    }).then(function (response) {
      var res = response.data;

      $this.ruleInfoList = res.value;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      $this.pageLoad = true;
      utils.loading(false);
    });
  },

  apiDelete: function (item) {
    var $this = this;

    utils.loading(true);
    $api.delete('', {
      params: {
        siteId: $this.siteId,
        ruleId: item.id
      }
    }).then(function (response) {
      var res = response.data;

      $this.ruleInfoList = res.value;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  },

  btnAddClick: function (item) {
    location.href = utils.getPageUrl('add.html');
  },

  btnEditClick: function (item) {
    location.href = utils.getPageUrl('add.html') + '&ruleId=' + item.id;
  },

  btnTestClick: function (ruleInfo) {
    utils.openLayer({
      url: utils.getPageUrl('rulesLayerTest.html') + '&ruleId=' + ruleInfo.id,
      title: '测试采集规则'
    });
  },

  btnChannelsClick: function (ruleInfo) {
    utils.openLayer({
      url: utils.getPageUrl('rulesLayerChannels.html') + '&ruleId=' + ruleInfo.id,
      title: '列表采集'
    });
  },

  btnContentsClick: function (ruleInfo) {
    utils.openLayer({
      url: utils.getPageUrl('rulesLayerContents.html') + '&ruleId=' + ruleInfo.id,
      title: '单页采集'
    });
  },

  btnEditClick: function (ruleInfo) {
    location.href = utils.getPageUrl('add.html') + '&ruleId=' + ruleInfo.id;
  },

  btnCopyClick: function (ruleInfo) {
    utils.openLayer({
      url: utils.getPageUrl('rulesLayerCopy.html') + '&ruleId=' + ruleInfo.id,
      title: '复制采集规则',
      width: 450,
      height: 300
    });
  },

  btnDeleteClick: function (ruleInfo) {
    var $this = this;

    swal2.alertDelete({
      title: '删除采集规则',
      text: '此操作将删除采集规则' + ruleInfo.gatherRuleName + '，确定吗？'
    }).then(function (result) {
      if (result.value) {
        $this.apiDelete(ruleInfo);
      }
    });
  }
};

new Vue({
  el: '#main',
  data: data,
  methods: methods,
  created: function () {
    this.apiGetList();
  }
});
