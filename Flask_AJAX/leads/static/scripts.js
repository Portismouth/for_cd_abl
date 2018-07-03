$(document).ready(function () {
  $('#data_form').on('submit', function (e) {
    e.preventDefault();
    $.ajax({
      url: $(this).attr('action'),
      method: 'post',
      data: $(this).serialize(),
      success: function (response) {
        $("#data").html(response);
      }
    });
    $(this).trigger('reset')
  });
  $('#search_form').keyup(function () {
    $.ajax({
      url: '/search',
      method: 'post',
      data: ($(this).serialize()),
      success: function (response) {
        $('#data').html(response);
      }
    })
  })
  $('body').on('submit', '.pagination-container form', function (e) {
    e.preventDefault();
    $.ajax({
      url: $(this).attr('action'),
      method: 'post',
      data: $(this).serialize(),
      success: function (response) {
        $("#data").html(response);
      }
    })
  })
})