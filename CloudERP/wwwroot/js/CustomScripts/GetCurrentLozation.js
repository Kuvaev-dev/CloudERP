function getLocation() {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(showPosition, showError);
    } else {
        alert("Geolocation is not supported by this browser.");
    }
}

function showPosition(position) {
    var lat = position.coords.latitude;
    var lon = position.coords.longitude;

    $.ajax({
        url: 'https://localhost:44311/api/addressapi/getaddressbycoordinates',
        data: { latitude: lat, longitude: lon },
        dataType: 'json',
        success: function (data) {
            if (data && data.features && data.features.length > 0) {
                var address = data.features[0].properties.formatted;
                $('#address').val(address);
            } else {
                alert("Не вдалося знайти адресу за координатами.");
            }
        },
        error: function (xhr, status, error) {
            console.error("Помилка під час запиту:", status, error);
            alert("Виникла помилка при отриманні адреси.");
        }
    });
}

function showError(error) {
    switch (error.code) {
        case error.PERMISSION_DENIED:
            alert("Користувач відхилив запит на геолокацію.");
            break;
        case error.POSITION_UNAVAILABLE:
            alert("Інформація про місцезнаходження недоступна.");
            break;
        case error.TIMEOUT:
            alert("Запит на визначення місцезнаходження перевищив час очікування.");
            break;
        case error.UNKNOWN_ERROR:
            alert("Невідома помилка.");
            break;
    }
}