function create(apiConfig) {
    var discovery = {};
    discovery.isApiConnected = function isApiConnected() {
        let type = typeof window[apiConfig.entryPoint];
        let exists = type !== "undefined";
        return exists;
    }

    discovery.findCallback = function findCallback(wrapFunction) {
        let entryPoint = window[apiConfig.entryPoint];
        if (!entryPoint) {
            error("entry point is undefined: " + apiConfig.entryPoint);
        }
        let callbackFunction = entryPoint[apiConfig.callbackName];
        if (!callbackFunction) {
            error("callback function is undefined: " + apiConfig.callbackName);
        }
        return wrapFunction
            ? function(name, eventObject) { callbackFunction(name, wrapFunction(eventObject)) }
            : callbackFunction;
    }

    discovery.consoleCallback = function consoleCallback() {
        var callbackObject = require("./callbackObject.js");
        return callbackObject.mutationCallback;
    }

    return discovery;
}

module.exports = { create };