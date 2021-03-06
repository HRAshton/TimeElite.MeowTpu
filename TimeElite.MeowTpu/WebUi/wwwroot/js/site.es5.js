﻿"use strict";
// ReSharper disable PossiblyUnassignedProperty

(function () {
    var $ = window.$;

    var countOfWeeksAfterCurrent = 1;

    $("document").on("load", function () {
        if ($(".today")[0]) {
            $(".today")[0].scrollIntoView();
        }
    });

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

    const generateButton = $("#generate-ical-btn");
    generateButton.on("click", function () {
        generateButton.disabled = true;
        generateButton.html("<img class=\"spinner\" style=\"width: 24px; height: 24px\" src=\"/images/loading.svg\"/>");

        const query = document.location.search.replace("?", "");
        if (query.includes(";1;")) {
            alert("Внимание\r\nЕсли включен режим \"Показывать окна\", то в календаре они тоже отобразятся");
        }
        fetch("/Api/GenerateIcalLink?" + query).then(function (response) {
            response.text().then(function (text) {
                const icalLinkInput = $("#ical-link");
                generateButton.hide();
                icalLinkInput.show();
                icalLinkInput.val(text);
            });
        });
    });

    $(".event").each(function (index, item) {
        const hideId = $(item).data("hideid");
        const currentLink = window.location.href;
 
        $(item).on("click", function () {
            if (currentLink.includes(hideId)) {
                window.location.href = window.location.href.replace(hideId, "").replace(",,", ",");
            } else {
                window.location.href += hideId + ",";
            }
        });
    });

    $("#next-page").on("click", function () {
        $("#matrix").empty();
        $("#matrix").append("<img class=\"spinner\" src=\"/images/loading.svg\"/>");

        const query = document.location.search.replace("?", "");
        fetch("/GetElementsForWeek?" + ++countOfWeeksAfterCurrent + ";" + query).then(function (response) {
            response.text().then(function (text) {
                $("#matrix").empty();
                $("#matrix").append(text);
            });
        });
    });

    $("#selector").selectize({
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
// ReSharper restore PossiblyUnassignedProperty

