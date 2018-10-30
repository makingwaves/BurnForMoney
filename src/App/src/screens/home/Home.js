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
  constructor(props) {
    super(props);

    this.state = {
      bfmStats: ''
    };
  }

  render() {
    return (
      <div className="Home">
        <VideoHeader/>
        <TotalNumbers data={this.state.bfmStats}/>
        <CurrentCharts data={this.state.bfmStats}/>
        <CharitySlider/>
        <TeamGoals/>
        <HowItWorks/>
        <InstaGallery/>
        <AboutEngagement/>
        <Footer/>
      </div>
    );
  }

  componentDidMount(){
    let api_url = process.env.REACT_APP_API_URL;
    fetch(api_url+"api/totalnumbers")
      .then(res => res.json())
      .then(
        (result) => {
          this.setState({
            bfmStats: result
          });
        },
        (error) => {
          this.setState({
            bfmStats: null,
          });
          console.error('Error:', error);
        }
      )
  }
}

export default Home;
