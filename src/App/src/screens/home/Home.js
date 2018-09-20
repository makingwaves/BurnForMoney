import React, { Component } from 'react';
import VideoHeader from './VideoHeader.js';
import TotalNumbers from './TotalNumbers.js';
import CurrentCharts from './CurrentCharts.js';
import CharitySlider from './CharitySlider.js';

class Home extends Component {
  render() {
    return (
      <div className="Home">
        <VideoHeader/>
        <TotalNumbers/>
        <CurrentCharts/>
        <CharitySlider/>
      </div>
    );
  }
}

export default Home;
