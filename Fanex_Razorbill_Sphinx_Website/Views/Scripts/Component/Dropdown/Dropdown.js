var Sphinx = Sphinx || {};

Sphinx.Component = function () {

};

Sphinx.Component.DropDownbox = new function () {
    this.listItem = {};

    function dropDownItem(value, img, text) {
        this.value = value;
        this.img = img;
        this.text = text;
    }

    this.options = {
        listItem: new Array(),
        selectedItem: ''

    };

    this.toggle = function () {

    };

    this.create = function (containerId, arrayItem, selectedItem, selectItemCallback) {

        if (document.getElementById(containerId) == null) {
            alert('Invalid dropdown container ' + containerId);
        }
        var container = $('#' + containerId);
        container.addClass('ddSelect');
        listItem = arrayItem || defaultData();
        var chosenItem = selectedItem || listItem[0];
        var containerWidth = container.width();
        container.append(dropdownTemplate(listItem, chosenItem, containerWidth, selectItemCallback));

        if (!browserStandardMode()) {
            container.width(containerWidth);
        }
        $('#selectedItemContainer').bind('click', function (event) {
            toggle();
            event.stopPropagation();
        });

        $('html').click(function () {
            $('#divItemList').hide();
        });

    };

    var toggle = function () {

        var divItemList = $('#divItemList');
        var divItemListIsHidden = divItemList.is(':hidden');
        if (divItemListIsHidden) {
            divItemList.show();
            return;
        }
        divItemList.hide();

    };

    var selectItem = function (chosenItem) {
        var selectedItem = $(chosenItem);
        var value = selectedItem.attr('value');
        var img = selectedItem.attr('img');
        var text = selectedItem.attr('text');
        var selectedItemContainer = $('#selectedItemContainer');
        selectedItemContainer.html('');
        selectedItemContainer.append(itemTemplate(value, img, text, true));

    };

    var dropdownTemplate = function (listItem, chosenItem, containerWidth, selectedCallback) {
        var htmlContent = $('<div></div>');

        var selectedItemContainer = $('<div id="selectedItemContainer"></div>');

        selectedItemContainer.append(itemTemplate(chosenItem.value, chosenItem.img, chosenItem.text, true));

        htmlContent.append(selectedItemContainer);

        htmlContent.append(browserStandardMode() ? '<div style="height: 12px;"></div>' : '');

        var divItemList = $('<div id="divItemList" style="display: none; "></div>');

        for (var i = 0; i < listItem.length; i++) {
            var item = listItem[i];
            divItemList.append(itemTemplate(item.value, item.img, item.text, false, selectedCallback));
        }

        divItemList.width(containerWidth);
        divItemList.css('z-index', 10);
        divItemList.css('position', 'absolute');
        divItemList.css('top', '24px');
        htmlContent.append(divItemList);
        return htmlContent;
    };
    var itemTemplate = function (value, img, text, selected, selectedCallback) {

        var selectedId = selected ? 'id="divSelectedItem"' : '';
        var caret = selected ? '<i class="caret"></i>' : '';

        // var onclick = selected ? '' : 'onclick=""';
        var height = selected ? 'height: 15px;' : '';

        var template = $('<div ' + selectedId + ' value="' + value + '" img="' + img + '" text="' + text + '" class="ddItem" style="' + height + ' background-color: #fff"> \
                                    <table>\
                                        <tr>\
                                            <td class="flag"><i class="' + img + '" id="flag"></i></td>\
                                            <td class="alignleft"><span id="lbLanguage">' + text + '</span></td>\
                                            <td class="alignright">' + caret + '</td>\
                                        </tr>\
                                    </table>\
                                </div>');
        template.click(function () {
            if (selected) return;
            selectItem(this);
            if (selectedCallback != null) {
                selectedCallback(value);
            }
        });

        return template;
    };

    var defaultData = function () {
        var items = new Array();
        items.push(new dropDownItem('en-US', 'flag-en', 'English'));
        items.push(new dropDownItem('vi-VN', 'flag-vn', 'Vietnamese'));
        items.push(new dropDownItem('zh-CN', 'flag-cn', 'Chinese'));
        items.push(new dropDownItem('zh-TW', 'flag-tw', 'Taiwan'));
        items.push(new dropDownItem('ko-KR', 'flag-kr', 'Korean'));
        items.push(new dropDownItem('ja-JP', 'flag-jp', 'Japanese'));
        items.push(new dropDownItem('th-TH', 'flag-th', 'Thailand'));
        return items;
    };

    var browserStandardMode = function () {
        return document.compatMode == 'CSS1Compat';
    };
};