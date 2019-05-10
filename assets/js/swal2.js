var swal2 = swal.mixin({
  confirmButtonClass: 'btn btn-primary',
  cancelButtonClass: 'btn btn-default ml-2',
  buttonsStyling: false,
});

swal2.success = function(title) {
  swal2({
    toast: true,
    type: 'success',
    title: title,
    showConfirmButton: false,
    timer: 2000
  });
};

swal2.success = function(title) {
  swal2({
    toast: true,
    type: 'success',
    title: title,
    showConfirmButton: false,
    timer: 2000
  });
};

swal2.error = function(title) {
  swal2({
    toast: true,
    type: 'error',
    title: title,
    showConfirmButton: false,
    timer: 2000
  });
};

swal2.alertDelete = function (config) {
	return swal2({
		title: config.title,
		text: config.text,
		type: 'question',
		confirmButtonText: config.confirmButtonText || '确定删除',
		confirmButtonClass: 'btn btn-danger',
		showCancelButton: true,
		cancelButtonText: '取 消'
	});
};