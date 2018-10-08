import React, { Component } from 'react';
import VideoHeader from './VideoHeader.js';
import TotalNumbers from './TotalNumbers.js';
import CurrentCharts from './CurrentCharts.js';
import CharitySlider from './CharitySlider.js';
import TeamGoals from './TeamGoals.js';
import HowItWorks from './HowItWorks.js';
import InstaGallery from './InstaGallery.js';
import AboutEngagement from './AboutEngagement.js';
import Footer from 'components/Footer.js';

class Home extends Component {
  render() {
    return (
      <div className="Home">
        <VideoHeader/>
        <TotalNumbers/>
        <CurrentCharts/>
        <CharitySlider/>
        <TeamGoals/>
        <HowItWorks/>
        <InstaGallery/>
        <AboutEngagement/>
        <Footer/>
      </div>
    );
  }
}

export default Home;
