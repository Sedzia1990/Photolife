function getHoverSrc(img) {
    return img.src.substring(0, img.src.length - 4) + '_h' + img.src.substring(img.src.length - 4, img.src.length);
}
function getNormalSrc(img) {
    return img.src.substring(0, img.src.length - 6) + '' + img.src.substring(img.src.length - 4, img.src.length);
}
$('document').ready(function () {

    var imgCache = new Array;
    $('.hoverable').each(function (index, elem) {
        var img = new Image;
        img.src = getHoverSrc(elem);
        imgCache.push(img);
    })

    $('.hoverable').mouseover(function () {
        this.src = getHoverSrc(this);
    });
    $('.hoverable').mouseout(function () {
        this.src = getNormalSrc(this);
    });
});

