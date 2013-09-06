function genApi(div, loading, api) {

    var root = div.parent();
    div.remove();
    div.addClass("main");

    var main_title = $("<div/>").addClass("main_title").html(api.Documentation);
    div.append(main_title);

    var main_body = $("<div/>").addClass("main_body");
    div.append(main_body);

    var main_directory = $("<div/>").addClass("main_directory");
    main_body.append(main_directory);

    var main_method = $("<div/>").addClass("main_method");
    main_body.append(main_method);

    var method_div_top = $("<div/>").attr("id", "method_div_top");
    main_method.append(method_div_top);

    var list_button = $("<img/>").attr("src", "?res=list.png").attr("id", "list_button").attr("title","收起").tipsy({ fade: true, gravity: 'n' });
    main_title.append(list_button);

    var user_div = $("<div/>").attr("id", "user_div");
    main_title.append(user_div);

    var user_label = $("<label/>");
    var user_button = $("<img/>").attr("src", "?res=user.png").attr("id", "user_button").attr("title", "当前用户").tipsy({ fade: true, gravity: 'n' })
    user_div.append(user_button).append(user_label).hide();

    //var user_label = $("<label/>");
    //var user_text = $("<input type='text' maxlength='100'/>").attr("id", "user_text").blur(function () {
    //    var debug_user = $(this).val().trim();
    //    user_label.text(debug_user);
    //    main_method.children(".method_div").children(".method_title:first").find("form:first").each(function () {
    //        var form = $(this);
    //        if (debug_user.length > 0) {
    //            form.attr("action", form.data("origin_url") + "&user=" + debug_user);
    //        }
    //        else {
    //            form.attr("action", form.data("origin_url"));
    //        }
    //    });
    //    user_text.animate({ width: "toggle" }, "fast");
    //    user_label.fadeIn("fast");
    //});

    //var user_button = $("<img/>").attr("src", "?res=user.png").attr("id", "user_button").click(function () {
    //    if (!user_text.is(":visible")) {
    //        user_text.animate({ width: "toggle" }, "fast");
    //        user_label.fadeOut("fast");
    //        user_text.focus();
    //    }
    //});

    //user_div.append(user_text).append(user_button).append(user_label);
    //user_text.hide();

    list_button.click(function () {
        if ($(main_directory).is(":animated") || $(main_method).is(":animated")) return;
        if (main_directory.is(":visible")) {
            list_button.attr("title", "展开").tipsy("show");
            main_directory.fadeOut("fast", function () {
                main_method.animate({ width: "100%" }, "fast");
            });
        }
        else {
            list_button.attr("title", "收起").tipsy("show");
            main_method.animate({ width: "75%" }, "fast", function () {
                main_directory.fadeIn("fast", function () {
                });
            });
        }
    });

    var ul = $("<ul/>");
    main_directory.append(ul);

    var total = api.Methods.length;
    var index = 0;

    for (var i = 0; i < total; i++) {
        loading.delay(0);
        loading.queue(function () {
            var method = api.Methods[index];

            var methodDiv = $("<div/>").addClass("method_div").attr("id", "mid_" + method.Name);
            genMethod(methodDiv, method, index);

            main_method.append(methodDiv);

            var nav = $("<div/>").text(method.Name).click(function () {
                var dy = main_method.find("#mid_" + method.Name).offset().top - method_div_top.offset().top;
                main_method.animate({ scrollTop: dy }, 120);
            });

            var li = $("<li/>").append(nav).attr("title", method.Documentation).tipsy({ fade: true, gravity: 'n' });
            ul.append(li);

            ++index;
            loading.dequeue();

            if (index == total) {
                loading.fadeOut("fast");
                root.append(div);
                refreshUser();
                if (ZeroClipboard.detectFlashSupport()) {
                    var copy_buttons = main_method.find(".copy_button")
                    var clip = new ZeroClipboard(copy_buttons);
                    clip.on("load", function (t) {
                        return $(t.htmlBridge).tipsy();
                    });
                    clip.on("complete", function (t) {
                        var e;
                        return e = $(t.htmlBridge),
                        e.prop("title", "复制成功！"),
                        e.tipsy("show");
                    });
                }
                else {
                    main_method.find(".copy_button").hide(); 
                }
            }
        });
    }
}

function refreshUser() {
    $.ajax({
        url: "?m=",
        type: "POST",
        success: function (data) {
            var user = data.User;
            var user_div = $("#user_div");
            var label = user_div.children("label:last");
            if (user == null || !user.IsAuthenticated) {
                label.text("");
                user_div.hide();
            }
            else {
                label.text(user.Name);
                user_div.show();
            }
        }
    });
}

function genMethod(div, method, index) {
    var url = "?m=" + method.Name;

    var method_title = $("<div/>").addClass("method_title");
    method_title.append($("<div/>").html((index + 1) + ". " + method.Documentation));

    var container = $("<div/>");
    method_title.append(container);

    var form = $("<form target='_blank' method='POST'/>").attr("action", url).data("origin_url", url);
    if (method.NeedAuth) {
        var lock = $("<span class='lock'/>").attr("title", "此接口需要验证").tipsy({ fade: true, gravity: 'n' });
        form.append(lock);
    }

    var postButton = $("<input type='submit' value='POST'/>").addClass("post_button").attr("disabled", true).click(function () {
        window.setTimeout(function () {
            refreshUser();
        }, 1000);
    });
    form.append(postButton);
    var a = $("<a/>").text(window.location.pathname + url);
    form.append(a);

    var copy_button = $("<span/>").addClass("copy_button").attr("data-clipboard-text", a.text()).attr("title", "复制链接");
    form.append(copy_button);

    var textarea = $("<textarea name='key1' spellcheck='false'></textarea>").hide();
    form.append(textarea);

    container.append(form);

    div.append(method_title);

    if (method.Params.length == 0) {
        div.append("<div class='sub_title'>传递参数 无</div>");
        a.attr("href", url).attr("target", "_blank");
    }
    else {
        a.attr("href", "javascript:void(0)").attr("target", "_self");
        div.append("<div class='sub_title'>传递参数</div>");

        var inputDiv = $("<div/>");
        genTable(inputDiv, method.Params);
        div.append(inputDiv);

        a.click(function () {
            if ($(textarea).is(":animated")) return;
            if (textarea.is(":visible")) {
                textarea.slideUp("fast");
                postButton.attr("disabled", true);
            }
            else {
                textarea.slideDown("fast");
                postButton.attr("disabled", false);
                genRequest();
            }
            inputDiv.find(".demo_value_cell").animate({ width: ["toggle", "linear"] }, "fast");
        });

        var inputs = inputDiv.find(".demo_value_input").keyup(genRequest);

        function genRequest() {
            var req = {};
            var hasError = false;
            for (var i = 0; i < method.Params.length; i++) {
                var param = method.Params[i];
                var demo_value_input = $(inputs[i]);
                var value = demo_value_input.val();
                var parsedValue;
                try {
                    if (param.Def.Name == "integer") {
                        if (param.Def.CanNull && value == "") parsedValue = null;
                        else {
                            parsedValue = parseInt(value);
                            if (window.isNaN(parsedValue)) throw "FormatError: Must be integer";
                        }
                    }
                    else if (param.Def.Name == "float") {
                        if (param.Def.CanNull && value == "") parsedValue = null;
                        else {
                            parsedValue = parseFloat(value);
                            if (window.isNaN(parsedValue)) throw "FormatError: Must be float";
                        }
                    }
                    else if (param.Def.Name == "boolean") {
                        if (param.Def.CanNull && value == "") parsedValue = null;
                        else {
                            var value = value.toLowerCase();
                            if (value == "false" || value == "no" || value == "0") parsedValue = false;
                            else parsedValue = Boolean(value);
                        }
                    }
                    else if (param.Def.Name == "object" || param.Def.Name == "array") {
                        if (value == "") parsedValue = null;
                        else parsedValue = JSON.parse(value);
                    }
                    else parsedValue = value;
                } catch (e) {
                    hasError = true;
                    demo_value_input.addClass("error");
                    demo_value_input.attr("title", e);
                    continue;
                }
                demo_value_input.removeAttr("title");
                demo_value_input.removeClass("error");
                req[param.Name] = parsedValue;
            }
            if (hasError) return;
            form.children("textarea:first").val(JSON.stringify(req));
        }
    }

    div.append("<div class='sub_title'>返回字段</div>");
    var outputDiv = div.append("<div></div>").children("div:last");
    genTable(outputDiv, method.ResponseParam.Properties);

    div.append("<div class='sub_title'>返回码说明</div>");
    var responseCodeDiv = div.append("<div></div>").children("div:last");
    genResponseCodeTable(responseCodeDiv, method.ResponseCodes);
}

function genTable(div, items) {
    var tbody = div.append("<table border='0' cellspacing='0' cellpadding='0' align='center' class='ptable'><tbody></tbody></table>").find("tbody:first");

    tbody.append("<tr class='title'><td>字段</td><td class='demo_value_cell'>值</td><td td width='150'>类型</td><td width='75'>可空</td><td>说明</td></tr>");

    for (var i = 0; i < items.length; i++) {
        var item = items[i];
        genItem(tbody, item);
    }
    div.find(".demo_value_cell").hide();
}

function genItem(tbody, item) {
    var row = $("<tr/>").addClass("row_item");
    tbody.append(row);
    var nameCell = $("<td/>").text(item.Name).addClass("name_cell");
    row.append(nameCell);
    var padLeft = 25 * item.Depth;
    if (padLeft > 0) {
        nameCell.css("padding-left", function (index, value) {
            var origin = parseInt(value);
            if (isFinite(origin)) return origin + padLeft;
            else return padLeft;
        });
    }

    var demo_value_cell = $("<td/>").addClass("demo_value_cell");
    row.append(demo_value_cell);

    if (item.Depth == 0) {
        var demo_value_input = $("<input class='demo_value_input' spellcheck='false' type='text'/>").val(item.Def.DemoValue);
        demo_value_cell.append(demo_value_input);
    }

    var start = tbody.children().length;

    if (item.Def.IsArray && item.SubType != null && item.SubType.Def.IsCustomObject && item.SubType.Properties != null) {
        for (var i = 0; i < item.SubType.Properties.length; i++) {
            genItem(tbody, item.SubType.Properties[i]);
        }
    }
    else if (item.Properties != null) {
        for (var i = 0; i < item.Properties.length; i++) {
            genItem(tbody, item.Properties[i]);
        }
    }

    var end = tbody.children().length;

    if (item.Def.IsCustomObject) {
        var span = row.append("<td class='type_cell'><span></span></td>").find("span:last");
        genSpanDef(tbody, span, item.Def, start, end);
    }
    else if (item.Def.IsArray && item.SubType != null) {
        var span = row.append("<td class='type_cell'>" + item.Def.Name + " of <span></span></td>").find("span:last");
        genSpanDef(tbody, span, item.SubType.Def, start, end);
    }
    else
        row.append("<td class='type_cell'>" + item.Def.Name + "</td>");

    var can_null_cell = $("<td/>").addClass("can_null_cell").text(item.Def.CanNull);
    row.append(can_null_cell);

    var doc_cell = $("<td/>").addClass("doc_cell").text(item.Documentation);
    row.append(doc_cell);
}

function genSpanDef(tbody, span, def, start, end) {
    span.text(def.Name);
    span.attr("tid", def.Id);
    if (!def.IsCustomObject) return;
    var range = end - start;
    if (range > 0) {
        span.addClass("tid_has_prop");
        span.data("start", start);
        span.data("end", end);
    }

    span.hover(function () {
        show_hilight($(this), true);
    }, function () {
        show_hilight($(this), false);
    });

    function show_hilight(span, show) {
        var items = tbody.find("span[tid='" + def.Id + "']");
        var rows;
        if (span.hasClass("tid_has_prop")) {
            rows = tbody.children("tr").slice(span.data("start") - 1, span.data("end"));
        }
        else {
            var propSpan = items.filter(".tid_has_prop:first");
            rows = tbody.children("tr").slice(propSpan.data("start") - 1, propSpan.data("end"));
        }
        var row_hilight = rows.slice(1, rows.length);
        var row_hilight_out = rows.first();

        if (show) {
            items.addClass("tid_hilight");
            row_hilight_out.addClass("row_hilight_bottom");
            row_hilight.addClass("row_hilight");
            row_hilight.last().addClass("row_hilight_bottom");
        }
        else {
            items.removeClass("tid_hilight");
            row_hilight_out.removeClass("row_hilight_bottom");
            row_hilight.removeClass("row_hilight");
            row_hilight.last().removeClass("row_hilight_bottom");
        }
    }
}

function genResponseCodeTable(div, items) {
    var tbody = div.append("<table border='0' cellspacing='0' cellpadding='0' align='center' class='ptable'><tbody></tbody></table>").find("tbody:first");

    tbody.append("<tr class='title'><td>返回码</td><td>说明</td></tr>");

    for (var i = 0; i < items.length; i++) {
        var item = items[i];
        var row = tbody.append("<tr></tr>").children("tr:last");
        row.addClass("row_item");
        row.append("<td>" + item.Code + "</td>");
        if (item.Category == 1) {
            if (item.Code == 200) {
                row.children("td:last").css("color", "green");
            }
            else {
                row.children("td:last").css("color", "red");
            }
        }

        if (item.Description != null)
            row.append("<td>" + item.Description + "</td>");
        else
            row.append("<td></td>");
    }
}