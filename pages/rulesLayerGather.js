var $api = axios.create({
  baseURL: utils.getQueryString('apiUrl') + '/' + utils.getQueryString('pluginId') + '/pages/rulesLayerGather/',
  withCredentials: true,
  params: {
    siteId: utils.getQueryInt('siteId'),
    ruleId: utils.getQueryInt('ruleId')
  }
});

var data = {
  pageLoad: false,
  pageAlert: null,
  pageGather: false,
  ruleInfo: null,
  channels: null,
  channelId: null,
  gatherNum: null,
  gatherUrlIsCollection: null,
  gatherUrlIsSerialize: null,
  gatherUrlCollection: null,
  gatherUrlSerialize: null,
  serializeFrom: null,
  serializeTo: null,
  serializeInterval: null,
  serializeIsOrderByDesc: null,
  serializeIsAddZero: null,
  urlInclude: null,
  guid: null,
  cache: {},
  percentage: null,
};

var methods = {
  apiGather: function () {
    this.pageGather = true;
    var $this = this;

    $api.post('actions/gather', {
      guid: this.guid
    }).then(function (response) {
      var res = response.data;

      $this.apiGetStatus();
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  },
  
  apiGetStatus: function() {
    var $this = this;

    $api.post('actions/getStatus', {
      guid: this.guid
    }).then(function (response) {
      var res = response.data;

      $this.cache = res.value || {};
      if ($this.cache.totalCount > 0) {
        $this.percentage = (($this.cache.successCount/$this.cache.totalCount) * 100).toFixed(1) + '%';
      }else {
        $this.percentage = '';
      }
      
      if ($this.cache.status === 'progress') {
        $this.apiGetStatus();
      }
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  },

  apiGet: function () {
    var $this = this;

    if ($this.pageLoad) utils.loading(true);
    $api.get('').then(function (response) {
      var res = response.data;

      $this.ruleInfo = res.value;
      $this.channels = res.channels;
      $this.channelId = $this.ruleInfo.channelId;
      $this.gatherNum = $this.ruleInfo.gatherNum;
      $this.gatherUrlIsCollection = $this.ruleInfo.gatherUrlIsCollection;
      $this.gatherUrlIsSerialize = $this.ruleInfo.gatherUrlIsSerialize;
      $this.gatherUrlCollection = $this.ruleInfo.gatherUrlCollection;
      $this.gatherUrlSerialize = $this.ruleInfo.gatherUrlSerialize;
      $this.serializeFrom = $this.ruleInfo.serializeFrom;
      $this.serializeTo = $this.ruleInfo.serializeTo;
      $this.serializeInterval = $this.ruleInfo.serializeInterval;
      $this.serializeIsOrderByDesc = $this.ruleInfo.serializeIsOrderByDesc;
      $this.serializeIsAddZero = $this.ruleInfo.serializeIsAddZero;
      $this.urlInclude = $this.ruleInfo.urlInclude;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      $this.pageLoad = true;
      utils.loading(false);
    });
  },

  apiSubmit: function () {
    var $this = this;

    utils.loading(true);
    $api.post('', {
      channelId: this.channelId,
      gatherNum: this.gatherNum,
      gatherUrlIsCollection: this.gatherUrlIsCollection,
      gatherUrlIsSerialize: this.gatherUrlIsSerialize,
      gatherUrlCollection: this.gatherUrlCollection,
      gatherUrlSerialize: this.gatherUrlSerialize,
      serializeFrom: this.serializeFrom,
      serializeTo: this.serializeTo,
      serializeInterval: this.serializeInterval,
      serializeIsOrderByDesc: this.serializeIsOrderByDesc,
      serializeIsAddZero: this.serializeIsAddZero,
      urlInclude: this.urlInclude,
    }).then(function (response) {
      var res = response.data;
      $this.guid = res.value;

      $this.apiGather();
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  },

  btnSubmitClick: function() {
    this.pageAlert = null;
    this.apiSubmit();
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
