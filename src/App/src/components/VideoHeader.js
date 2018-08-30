import React, { Component } from 'react';
import logo from '../img/logo.svg';
import bfmVideo from '../video/BFM.mp4';


class VideoHeader extends Component {
  render() {
    return (
      <div className="videoHeader">
        <video loop muted autoPlay poster={logo} className="videoHeader__video">
            <source src={bfmVideo} type="video/mp4"/>
        </video>
        <div className="videoHeader__content">
          <img src={logo} alt="Burn for money" className="videoHeader__logo" />
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
