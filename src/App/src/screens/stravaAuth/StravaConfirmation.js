import React, { Component } from 'react';

import './StravaAuth.css';
import logo from 'img/logo.svg';


class StravaAuth extends Component {
  constructor(props) {
      super(props);
      this.state = { stravaLink: undefined } ;
    }

    componentWillMount() {
      let stravaLink = process.env.REACT_APP_STRAVA_AUTH_PAGE;
      this.setState({ stravaLink });
    }

  render() {
    return (
      <div className="StravaAuth">
        <img src={logo} alt="Logo" className="logo"/>
        <p>Your account is synchronised!</p>
        <p>Now your points will be counted<br/>
        automatically for activity you log to Strava.</p>
        <div className="StravaAuth__confirmation">
          <div className="check_mark">
            <div className="sa-icon sa-success animate">
              <span className="sa-line sa-tip animateSuccessTip"></span>
              <span className="sa-line sa-long animateSuccessLong"></span>
              <div className="sa-placeholder"></div>
              <div className="sa-fix"></div>
            </div>
          </div>
        </div>
      </div>
    );
  }
}

export default StravaAuth;
