// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import React from 'react';
import './App.css';
import * as microsoftTeams from "@microsoft/teams-js";
import { BrowserRouter,  Route, Routes } from "react-router-dom";

import Privacy from "./Privacy";
import TermsOfUse from "./TermsOfUse";
import Tab from "./Tab";
import ConsentPopup from "./ConsentPopup";
import ClosePopup from "./ClosePopup";

/**
 * The main app which handles the initialization and routing
 * of the app.
 */
function App() {

  // Initialize the Microsoft Teams SDK
  microsoftTeams.app
  .initialize()
  .then(() => {
    console.log("App.js: initializing client SDK initialized");
    microsoftTeams.app.notifyAppLoaded();
    microsoftTeams.app.notifySuccess();
   // setInitialized(true);
  })
  .catch((error) => console.error(error));

  // Display the app home page hosted in Teams
  return (
    <BrowserRouter>
    <Routes>
      <Route path="/privacy" element={<Privacy/>} />
      <Route path="/termsofuse" element={<TermsOfUse/>} />
      <Route path="/auth-start" element={<ConsentPopup/>} />
      <Route path="/auth-end" element={<ClosePopup/>} />
      <Route path="/tab" element={<Tab/>} />
    </Routes>
    </BrowserRouter>
  );
}

export default App;
