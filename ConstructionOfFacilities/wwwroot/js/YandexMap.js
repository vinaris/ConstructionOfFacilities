"use strict";
var myMap;

var myScript = document.getElementById('YandexMapScript');

ymaps.ready(init);

function init () {
    var address = myScript.dataset.src;
    var myGeocoder = ymaps.geocode("Красноярск " + address);
    myGeocoder.then(
            function (res) {
                var Coords = res.geoObjects.get(0).geometry.getCoordinates();

                myMap = new ymaps.Map('map',
                    {
                        center: Coords,
                        zoom: 17
                    });
                myGeoObject = new ymaps.GeoObject({
                    geometry: {
                        type: "Point",
                        coordinates: [Coords[0], Coords[1]]
                    }
                });
                myMap.geoObjects
                    .add(myGeoObject);
            },
            function (err) {
                alert('Ошибка геолокации');
            });
    document.getElementById('destroyButton').onclick = function () {
        myMap.destroy();
    };

}