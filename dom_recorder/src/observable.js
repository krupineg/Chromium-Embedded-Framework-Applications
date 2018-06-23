"use strict";
var Callbacks = [];

function raise(eventName, eventObject) {
    Callbacks.forEach(function(callback) {
        callback(eventName, eventObject);
    });
}

function subscribe(callback) {
    Callbacks.push(callback);
}

module.exports = { raise, subscribe };