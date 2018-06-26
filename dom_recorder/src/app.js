"use strict";

let factory = require("./factory.js");
let config = require("./config/entry.config.json");
let domObserver = require("./observable.js");
let mutationAware = require("./mutation.js");

let api = factory.create(mutationAware, domObserver, config);
let callbackDiscovery = require("./callbackDiscovery.js").create(config);

/*
  let callbackFunction = CallbackDiscovery.isApiConnected(config)
      ? CallbackDiscovery.findCallback(config)
      : CallbackDiscovery.consoleCallback();
*/

module.exports = { callbackDiscovery, api };