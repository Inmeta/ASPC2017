// KnockoutJS extentions
ko.bindingHandlers.slideVisible = {
    update: function (element, valueAccessor, allBindings) {
        var value = ko.unwrap(valueAccessor());
        var duration = allBindings.get('slideDuration') || 400;

        if (value === true) {
            $(element).slideDown(duration);
        }
        else {
            $(element).slideUp(duration);
        }
    }
};

ko.bindingHandlers.datePicker = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        var datePickerOptions = {
            autoclose: true,
            calendarWeeks: true,
            format: "dd.mm.yyyy",
            language: "no",
            orientation: "bottom auto",
            todayHighlight: true
        };

        //initialize datepicker with some optional options
        var options = allBindingsAccessor().datePickerOptions || datePickerOptions;
        $(element).datepicker(options);

        //when a user changes the date, update the view model
        ko.utils.registerEventHandler(element, "changeDate", function (event) {
            var value = valueAccessor();
            if (ko.isObservable(value)) {
                if (event.date != null && !(event.date instanceof Date)) {
                    value(event.date.toDate());
                } else {
                    value(event.date);
                }
            }
        });

        ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
            var picker = $(element).data("datepicker");
            if (picker) {
                picker.destroy();
            }
        });
    },

    update: function (element, valueAccessor) {
        var value = ko.utils.unwrapObservable(valueAccessor());
        var picker = $(element).data("datepicker");

        if (picker) {
            picker.setDate(value);
        }
        else {
            $(element).val(value);
        }
    }
};