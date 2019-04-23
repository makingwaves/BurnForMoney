import React from 'react';
import {
  BrowserRouter as Router,
  Route,
  Switch
} from 'react-router-dom';

import Home from './apps/home/Home';
import BfmPanel from './apps/bfmPanel/BfmPanel';
import AppTvboard from './apps/tvboard/AppTvboard';



import {authComponent, SignInHandler, SilentCallbackHandler} from "./components/Authentication/AuthComponent";
import SimpleLoginPage from "./components/Authentication/SimpleLoginPage";

function App(){
  let authBfmPanel = authComponent(BfmPanel, SimpleLoginPage);
  let authTvboard = authComponent(AppTvboard, SimpleLoginPage);

  return (
    <Router>
      <div className="App">
        <Switch>
          <Route exact path="/" component={Home} />
          <Route path="/dashboard" component={authBfmPanel} /> //protect route
          <Route path="/tvboard" component={authTvboard} /> //protect route

          <Route path="/auth/signin" component={SignInHandler} />
          <Route path="/auth/silent" component={SilentCallbackHandler} />
        </Switch>
      </div>
    </Router>
  );
}

export default App;
