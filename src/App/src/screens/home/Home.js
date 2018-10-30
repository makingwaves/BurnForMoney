import React, { Component } from 'react';
import * as contentful from 'contentful';
import VideoHeader from './VideoHeader/VideoHeader.js';
import TotalNumbers from './TotalNumbers/TotalNumbers.js';
import CurrentCharts from './CurrentCharts/CurrentCharts.js';
import CharitySlider from './CharitySlider/CharitySlider.js';
import TeamGoals from './TeamGoals/TeamGoals.js';
import HowItWorks from './HowItWorks/HowItWorks.js';
import InstaGallery from './InstaGallery/InstaGallery.js';
import OtherInitiatives from './OtherInitiatives/OtherInitiatives.js';
import Footer from 'components/Footer/Footer.js';

class Home extends Component {
  constructor(props) {
    super(props);

    this.state = {
      bfmStats: '',
      contentful: ''
    };
  }

  render() {
    return (
      <div className="Home">
        <VideoHeader/>
        <TotalNumbers data={this.state.bfmStats}/>
        <CurrentCharts data={this.state.bfmStats}/>
        <CharitySlider data={this.state.contentful}/>
        <TeamGoals/>
        <HowItWorks/>
        <InstaGallery/>
        <OtherInitiatives/>
        <Footer/>
      </div>
    );
  }

  componentDidMount(){
    const api_contentful = process.env.REACT_APP_CONTENTFUL;
    const api_url = process.env.REACT_APP_API_URL;

    const client = contentful.createClient({
      space: "r9sx20y0suod",
      accessToken: "0cfdeec874152c24de8109da60c0bd09630fd3e4efdeddf9223652a433927fc4",
      host: "preview.contentful.com"
    });

    client.getEntries().then(entries => {

      this.setState({
        contentful: entries.items
      });
      console.log('contentful', this.state.contentful)
/*
      entries.items.forEach(entry => {
        if(entry.fields) {

          console.log('contentful', this.state.contentful);
        } else {
          this.setState({
            contentful: null
          });
        }
      })
*/
    })

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
      );
  }
}

export default Home;
