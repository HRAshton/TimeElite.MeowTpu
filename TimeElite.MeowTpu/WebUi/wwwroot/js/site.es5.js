"use strict";

(function () {
    $("#edit-legend").on("click", function () {
        $("#legend-show-block").css("display", "none");
        $("#legend-edit-block").css("display", "inline-flex");

        $(".events").each(function (_, item) {
            return $(item).children().remove();
        });
    });

    $("#save-legend").on("click", function () {
        var hashes = $("#selector").selectize().val();

        document.location.href = "/" + hashes;
    });

    var selector = $("#selector"); //TODO: only on edit pressed
    selector.selectize({
        valueField: "hash",
        labelField: "title",
        searchField: "html",
        // maxOptions: 10,
        maxItems: 12,
        create: false,
        // items: initialItems.map(item => item.hash),
        // options: initialItems,
        score: function score(item) {
            return function (item2) {
                return 1;
            };
        },
        render: {
            option: function option(item, escape) {
                return "<div>" + item.html + "</div>";
            }
        },
        load: function load(query, callback) {
            if (!query.length) return callback();
            $.ajax({
                url: "http://localhost:65222/api/Search?query=" + encodeURIComponent(query),
                type: "GET",
                error: function error() {
                    callback();
                },
                success: function success(res) {
                    $("#selector").selectize()[0].selectize.clearOptions();
                    callback(res);
                }
            });
        }
    });
})();

