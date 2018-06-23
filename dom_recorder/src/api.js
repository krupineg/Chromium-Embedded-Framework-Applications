"use strict";
var CallbackFunction = {};
var HighlightLibrary = {};
var IgnoreAttribute = {};

function init(callback, highlight, ignoreAttribute) {
    CallbackFunction = callback;
    HighlightLibrary = highlight;
    IgnoreAttribute = ignoreAttribute;
    return this;
}

function callback(name, obj) {
    CallbackFunction(name, obj);
}

function extinguish(x, y) {
    HighlightLibrary.extinguishAt(x, y, IgnoreAttribute);
}

function highlight(x, y, color) {
    HighlightLibrary.highlightAt(x, y, color, IgnoreAttribute);
}
module.exports = { init, callback, highlight, extinguish };