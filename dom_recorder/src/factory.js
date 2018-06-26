"use strict";
let highlightLibrary = require("./highlight.js");

function validateConfig(apiConfig) {
    if (!apiConfig.entryPoint) {
        errorAbout("entry point name", "entryPoint");
    }
    if (!apiConfig.ignoreAttribute) {
        errorAbout("ignore attribute", "ignoreAttribute");
    }
    if (!apiConfig.callbackName) {
        errorAbout("callback function", "callbackName");
    }
}

function errorAbout(configItem, configKey) {
    error(`${configItem} config entry is undefined, please add ${configKey} to dom recorder config json file.`);
}

function create(mutationAware, domObserver, apiConfig) {
    validateConfig(apiConfig);
    let api = require("./api.js");
    return api.init(mutationAware, domObserver, highlightLibrary);
}

module.exports = { create };