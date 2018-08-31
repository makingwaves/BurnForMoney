import React, { Component } from 'react';

import './VideoHeader.css';
import logo from 'img/logo.svg';
import bfmVideo from 'video/BFM.mp4';


class VideoHeader extends Component {
  render() {
    return (
      <div className="VideoHeader">
        <video loop muted autoPlay poster={logo} className="VideoHeader__video">
            <source src={bfmVideo} type="video/mp4"/>
        </video>
        <div className="VideoHeader__content">
          <img src={logo} alt="Burn for money" className="VideoHeader__logo" />
          <p>workout to support charity</p>
          <button onClick = {this.onButtonClick} >watch in action</button>
        </div>

      </div>
    );
  }

  onButtonClick(e){
    console.log('click!', e);
  }
}

export default VideoHeader;
