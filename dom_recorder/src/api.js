"use strict";
var HighlightLibrary = {};
var Observable = {};
var MutationAware = {};
let defaultColor = '#f00';

function init(mutationAware, observable, highlightLibrary) {
    Observable = observable;
    HighlightLibrary = highlightLibrary;
    MutationAware = mutationAware;
    return this;
}

function subscribe(callback, additionalAttributes) {
    Observable.subscribe(callback);
    MutationAware.observeMutation(Observable, additionalAttributes);
}

function extinguish() {
    HighlightLibrary.extinguish();
}

function highlight(x, y) {
    HighlightLibrary.highlightAt(x, y, defaultColor);
}

module.exports = { init, subscribe, highlight, extinguish };