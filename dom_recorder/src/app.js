"use strict";
let factory = require("./factory.js");
let config = require("./config/entry.config.json");
let domObserver = require("./observable.js");

let api = factory.create(config);
domObserver.subscribe(function (name, obj) {
    api.callback(name, JSON.stringify(obj));
});

let mutationAware = require("./mutation.js");
mutationAware.observeMutation(domObserver, config);

// if do we need to use api outside of this script.
window.api = api;