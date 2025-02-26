$(document).ready(function () {
    $('#address').on('input', function () {
        var query = $(this).val();
        if (query.length > 2) {
            $.ajax({
                url: '/Address/Autocomplete',
                data: { query: query },
                dataType: 'json',
                success: function (data) {
                    if (data && data.features) {
                        var suggestions = data.features;
                        $('#suggestions').empty();
                        suggestions.forEach(function (suggestion) {
                            $('#suggestions').append('<li class="suggestion-item">' + suggestion.properties.formatted + '</li>');
                        });
                    } else {
                        console.error("Unexpected JSON format:", data);
                    }
                },
                error: function (xhr, status, error) {
                    console.error("Error during AJAX request:", status, error);
                }
            });
        }
    });

    $(document).on('click', '.suggestion-item', function () {
        var selectedAddress = $(this).text();
        $('#address').val(selectedAddress);
        $('#suggestions').empty();
    });
});
