import React, { Component } from 'react';
import VideoHeader from './VideoHeader.js';
import BfmProgress from './BfmProgress.js';
import Slider from './Slider.js';

class Home extends Component {
  render() {
    return (
      <div className="Home">
        <VideoHeader/>
        <BfmProgress/>
        <Slider/>
      </div>
    );
  }
}

export default Home;
