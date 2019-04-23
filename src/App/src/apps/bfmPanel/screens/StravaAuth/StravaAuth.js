import React, { Component } from 'react';

import './StravaAuth.css';
import logo from 'static/img/logo.svg';
import arrow from './img/arrow_down.svg';
import bigButtonTop from './img/bigButton_top.svg';
import bigButtonBottom from './img/bigButton_bottom.svg';

class StravaAuth extends Component {
  constructor(props) {
    super(props);
    this.state = { stravaLink: undefined } ;
  }

  componentWillMount() {
    const stravaLink = process.env.REACT_APP_STRAVA_AUTH_PAGE;
    this.setState({ stravaLink });
  }

  render() {
    return (
      <div className="StravaAuth">
        <img src={logo} alt="Logo" className="logo"/>
        <p>Hi Strava user,<br/>
Authorize Burn For Money to connect to Strava,<br/>
so your points will be automatically counted in our stats.</p>
        <div className="StravaAuth__authorize">
          <p>Authorize</p>
          <img src={arrow} className="StravaAuth__authorize-indicator" alt="authorize below"/>
        </div>
        <a href={this.state.stravaLink} className="StravaAuth__bigButton">
          <img src={bigButtonTop} className="StravaAuth__bigButton-top" alt="big button top"/>
          <img src={bigButtonBottom} className="StravaAuth__bigButton-bottom" alt="big button base"/>
        </a>
      </div>
    );
  }
}

export default StravaAuth;
