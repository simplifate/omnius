var cislo1;
var cislo2;
var cislo3;
var cislo4;
var cislo5;
var cislo6;
var cislo7;
var cislo8;
$(function () {
  alert("test");
$("#GPU_ks_input").on("change", function (e) {
	 $("#GPU_componentprice_input").val(($(this).val()) * ($("#GPU_price_input").val()))});
  
$("#GPU_ks_input").on("change", function (e) {
  	$("#TotalPrice_input").val((($(this).val()) * ($("#GPU_price_input").val())) + (($("#CPU_price_input").val()) * ($("#CPU_ks_input").val())) + (($("#Motherboard_price_input").val()) * ($("#Motherboard_ks_input").val())) + (($("#RAM_ks_input").val()) * ($("#RAM_price_input").val())))});
  
$("#GPU_price_input").on("change", function (e) {
	 $("#GPU_componentprice_input").val(($(this).val()) * ($("#GPU_ks_input").val()))});

$("#GPU_price_input").on("change", function (e) {
  	$("#TotalPrice_input").val((($(this).val()) * ($("#GPU_ks_input").val())) + (($("#CPU_price_input").val()) * ($("#CPU_ks_input").val())) + (($("#Motherboard_price_input").val()) * ($("#Motherboard_ks_input").val())) + (($("#RAM_ks_input").val()) * ($("#RAM_price_input").val())))});
  
$("#CPU_ks_input").on("change", function (e) {
	$("#CPU_componentprice_input").val(($(this).val()) * ($("#CPU_price_input").val()))});
  
$("#CPU_ks_input").on("change", function (e) {
  	$("#TotalPrice_input").val((($(this).val()) * ($("#CPU_price_input").val())) + (($("#GPU_price_input").val()) * ($("#GPU_ks_input").val())) + (($("#Motherboard_price_input").val()) * ($("#Motherboard_ks_input").val())) + (($("#RAM_ks_input").val()) * ($("#RAM_price_input").val())))});
  
$("#CPU_price_input").on("change", function (e) {
	$("#CPU_componentprice_input").val(($(this).val()) * ($("#CPU_ks_input").val()))});
  
$("#CPU_price_input").on("change", function (e) {
  	$("#TotalPrice_input").val((($(this).val()) * ($("#CPU_ks_input").val())) + (($("#GPU_price_input").val()) * ($("#GPU_ks_input").val())) + (($("#Motherboard_price_input").val()) * ($("#Motherboard_ks_input").val())) + (($("#RAM_ks_input").val()) * ($("#RAM_price_input").val())))});
  
$("#Motherboard_ks_input").on("change", function (e) {
	 $("#Motherboard_componentprice_input").val(($(this).val()) * ($("#Motherboard_price_input").val()))});
  
$("#Motherboard_ks_input").on("change", function (e) {
  	$("#TotalPrice_input").val((($(this).val()) * ($("#Motherboard_price_input").val())) + (($("#GPU_price_input").val()) * ($("#GPU_ks_input").val())) + (($("#CPU_ks_input").val()) * ($("#CPU_price_input").val())) + (($("#RAM_ks_input").val()) * ($("#RAM_price_input").val())))});
  
$("#Motherboard_price_input").on("change", function (e) {
	 $("#Motherboard_componentprice_input").val(($(this).val()) * ($("#Motherboard_ks_input").val()))});
  
$("#Motherboard_price_input").on("change", function (e) {
  	$("#TotalPrice_input").val((($(this).val()) * ($("#Motherboard_ks_input").val())) + (($("#GPU_price_input").val()) * ($("#GPU_ks_input").val())) + (($("#CPU_ks_input").val()) * ($("#CPU_price_input").val())) + (($("#RAM_ks_input").val()) * ($("#RAM_price_input").val())))});
 
$("#RAM_ks_input").on("change", function (e) {
	 $("#RAM_componentprice_input").val(($(this).val()) * ($("#RAM_price_input").val()))});
  
$("#RAM_ks_input").on("change", function (e) {
  	$("#TotalPrice_input").val((($(this).val()) * ($("#RAM_price_input").val())) + (($("#GPU_price_input").val()) * ($("#GPU_ks_input").val())) + (($("#CPU_ks_input").val()) * ($("#CPU_price_input").val())) + (($("#Motherboard_ks_input").val()) * ($("#Motherboard_price_input").val())))});

$("#RAM_price_input").on("change", function (e) {
	 $("#RAM_componentprice_input").val(($(this).val()) * ($("#RAM_ks_input").val()))});
  
$("#RAM_price_input").on("change", function (e) {
  	$("#TotalPrice_input").val((($(this).val()) * ($("#RAM_ks_input").val())) + (($("#GPU_price_input").val()) * ($("#GPU_ks_input").val())) + (($("#CPU_ks_input").val()) * ($("#CPU_price_input").val())) + (($("#Motherboard_ks_input").val()) * ($("#Motherboard_price_input").val())))});
});