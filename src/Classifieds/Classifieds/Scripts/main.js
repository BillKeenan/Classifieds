$("div.item-preview").hover(function()
{
    $(this).children("div.centeredTitle").show();
},
function () {
    $(this).children("div.centeredTitle").hide();
})