var $api = axios.create({
  baseURL: utils.getQueryString('apiUrl') + '/' + utils.getQueryString('pluginId') + '/pages/rulesLayerContents/',
  withCredentials: true,
  params: {
    siteId: utils.getQueryInt('siteId'),
    ruleId: utils.getQueryInt('ruleId')
  }
});

var $urlImport = 'actions/import';

var data = {
  pageLoad: false,
  pageAlert: null,
  pageGather: false,
  ruleInfo: null,
  channels: null,
  channelId: null,
  gatherType: 'url',
  urls: null,
  fileName: null,
  uploadUrl: null,
  files: [],
  guid: null,
  cache: {},
  percentage: null,
};

var methods = {
  apiGather: function () {
    this.pageGather = true;
    var $this = this;

    $api.post('actions/gather', {
      guid: this.guid,
      channelId: this.channelId,
      gatherType: this.gatherType,
      urls: this.urls,
      fileName: this.fileName
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
      $this.guid = res.guid;
      $this.uploadUrl = $api.defaults.baseURL + $urlImport + '?adminToken=' + res.adminToken + '&siteId=' + utils.getQueryInt('siteId');
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      $this.pageLoad = true;
      utils.loading(false);
    });
  },

  inputFile(newFile, oldFile) {
    var $this = this;

    if (Boolean(newFile) !== Boolean(oldFile) || oldFile.error !== newFile.error) {
      if (!this.$refs.import.active) {
        this.$refs.import.active = true
      }
    }

    if (newFile && oldFile && newFile.xhr && newFile.success !== oldFile.success) {
      this.fileName = newFile.name;
      swal2({
        title: 'excel文件导入成功',
        type: 'success',
        confirmButtonText: '开始采集',
        confirmButtonClass: 'btn btn-primary',
      }).then(function (result) {
        if (result.value) {
          $this.apiGather();
        }
      });
    }
  },

  inputFilter: function (newFile, oldFile, prevent) {
    if (newFile && !oldFile) {
      if (!/\.(xls)|\.(xlsx)|\.(csv)$/i.test(newFile.name)) {
        swal2({
          title: '上传格式错误！',
          text: '请上传excel文件',
          type: 'error',
          confirmButtonText: '确 定',
          confirmButtonClass: 'btn btn-primary',
        });
        return prevent()
      }
    }
  },

  btnSubmitClick: function() {
    this.pageAlert = null;

    var $this = this;
    this.$validator.validate().then(function (result) {
      if (result) {
        $this.apiGather();
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
  components: {
    FileUpload: VueUploadComponent
  },
  methods: methods,
  created: function () {
    this.apiGet();
  }
});
