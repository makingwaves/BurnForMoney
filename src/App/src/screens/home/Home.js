import React, { Component } from 'react';
import VideoHeader from './VideoHeader/VideoHeader.js';
import TotalNumbers from './TotalNumbers/TotalNumbers.js';
import CurrentCharts from './CurrentCharts/CurrentCharts.js';
import CharitySlider from './CharitySlider/CharitySlider.js';
import TeamGoals from './TeamGoals/TeamGoals.js';
import HowItWorks from './HowItWorks/HowItWorks.js';
import InstaGallery from './InstaGallery/InstaGallery.js';
import AboutEngagement from './AboutEngagement/AboutEngagement.js';
import Footer from 'components/Footer/Footer.js';

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
