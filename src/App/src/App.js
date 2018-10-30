import React, { Component } from 'react';
import {
  BrowserRouter as Router,
  Route
} from 'react-router-dom';

import Home from './screens/home/Home';
import StravaAuth from './screens/stravaAuth/StravaAuth';


class App extends Component {
  render() {
    return (
      <Router>
        <div className="App">
          <Route exact path="/" component={Home} />
          <Route path="/strava" component={StravaAuth} />
        </div>
      </Router>
    );
  }
}

export default App;
