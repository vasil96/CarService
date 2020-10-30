// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(function () {
    var bodyHeight = window.innerHeight - $('footer').height() - $('.navbar-fixed-top').height();
    $('body').css('min-height', bodyHeight + 'px');
})