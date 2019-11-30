"use strict";

(function () {
    $("#clear-legend").on("click", function () {
        $("#legend-show-block").css("display", "none");
        $("#legend-edit-block").css("display", "inline-flex");

        $(".events").each(function (_, item) {
            return $(item).children().remove();
        });
        document.cookie = "link=; expires=Thu, 01 Jan 1970 00:00:01 GMT;";
    });

    $("form").on("submit", function (e) {
        var hashes = $("#selector").selectize().val();
        var link = "0;0;" + hashes + ";";

        document.location.href = "/?" + link;

        e.preventDefault();
        return false;
    });

    $("#save-legend").on("click", function () {
        $("form").submit();
    });

    $(".event").each(function (index, item) {
        var hideId = $(item).data("hideid");
        var currentLink = window.location.href;

        $(item).on("click", function (e) {
            if (currentLink.includes(hideId)) {
                window.location.href = window.location.href.replace(hideId, "").replace(",,", ",");
            } else {
                window.location.href += hideId + ",";
            }
        });
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
                url: window.location.origin + "/api/Search?query=" + encodeURIComponent(query),
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

