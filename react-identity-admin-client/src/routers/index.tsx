import React from "react";
import { BrowserRouter, Route } from "react-router-dom";

import App from "../features/App";

const Root = () => (
  <BrowserRouter key="browser-router">
    <Route path="/*" component={App} key="route" />
  </BrowserRouter>
);

export default Root;
