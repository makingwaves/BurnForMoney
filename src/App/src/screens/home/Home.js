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
    let auth_code = process.env.REACT_APP_AUTH_CODE;
    fetch(api_url+"api/totalnumbers?code="+auth_code)
      .then(res => res.json())
      .then(
        (result) => {
          console.log('bfmStats JSON:',result);
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
    /*
    const json = {
      "distance": 453232,
      "time": 1397,
      "money": 95,
      "thisMonth": {
          "numberOfTrainings": 36,
          "percentOfEngagedEmployees": 37,
          "money": 95,
          "mostFrequentActivities": [
              {
                  "category": "Ride",
                  "numberOfTrainings": 32,
                  "points": 438
              },
              {
                  "category": "Run",
                  "numberOfTrainings": 3,
                  "points": 31
              },
              {
                  "category": "Gym",
                  "numberOfTrainings": 1,
                  "points": 6
              }
          ]
      }
    }
    */
}

export default Home;
