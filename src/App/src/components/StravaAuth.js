import React, { Component } from 'react';
import logo from '../img/logo.svg';

import arrow from '../img/arrow_down.svg';
import bigButtonTop from '../img/bigButton_top.svg';
import bigButtonBottom from '../img/bigButton_bottom.svg';

class StravaAuth extends Component {
  render() {
    return (
      <div className="strava-contanier">
        <img src={logo} alt="Logo" className="logo"/>
        <p>Hi Strava user,<br/>
Authorize Burn For Money to connect to Strava,<br/>
so your points will be automatically counted in our stats.</p>
        <div className="authorizeIndicator">
          <p>Authorize</p>
          <img src={arrow} className="authorizeArrow" alt="authorize below"/>
        </div>
        <a href="#" className="bigButton">
          <img src={bigButtonTop} className="bigButtonTop" alt="big button top"/>
          <img src={bigButtonBottom} className="bigButtonBottom" alt="big button base"/>
        </a>
      </div>
    );
  }
}

export default StravaAuth;
