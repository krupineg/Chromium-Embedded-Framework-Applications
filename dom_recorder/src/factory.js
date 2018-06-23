"use strict";
function validateConfig(apiConfig) {
    if (!apiConfig.entryPoint) {
        error("entry point config is undefined, please add entryPoint key to dom recorder config json file.");
    }
    if (!apiConfig.callbackName) {
        error("callback function config is undefined, please add callbackName key to dom recorder config json file.");
    }
}

function isApiConnected(apiConfig) {
    let type = typeof window[apiConfig.entryPoint];
    let exists = type !== "undefined";
    return exists;
}

function findCallback(apiConfig) {
    let entryPoint = window[apiConfig.entryPoint];
    if (!entryPoint) {
        error("entry point is undefined: " + apiConfig.entryPoint);
    }
    let callbackFunction = entryPoint[apiConfig.callbackName];
    if (!callbackFunction) {
        error("callback function is undefined: " + apiConfig.callbackName);
    }
    return callbackFunction;
}

function consoleCallback(apiConfig) {
    var callbackObject = require("./callbackObject.js");
    return callbackObject.mutationCallback;
}

function create(apiConfig) {
    validateConfig(apiConfig);
    let callbackFunction = isApiConnected(apiConfig) ? findCallback(apiConfig) : consoleCallback();
    let highlightLibrary = require("./highlight.js");
    let api = require("./api");
    return api.init(callbackFunction, highlightLibrary, apiConfig.ignoreAttribute);
}

module.exports = { create };
