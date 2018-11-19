import React, { Component } from 'react';

import './Home.css';
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

import i18n from 'i18n';
import { withNamespaces } from 'react-i18next';


class Home extends Component {
  constructor(props) {
    super(props);

    this.state = {
      bfmStats: '',
      contentful: '',
      lang: localStorage.getItem('language') || 'en'
    };
  }

  render() {
    console.log(this.state.lang);
    const changeLanguage = (lng) => {
      this.setState({
        lang: lng
      });
      localStorage.setItem('language', lng);

      i18n.changeLanguage(lng);
    }

    return (
      <div className="Home">
        <div className="Home__langSwitcher">
          <button className={`Home__langSwitcher-Button ${(this.state.lang == 'en' ? 'active' : '')}`} onClick={() => changeLanguage('en') }>en</button>
          <button className={`Home__langSwitcher-Button ${(this.state.lang == 'pl' ? 'active' : '')}`} onClick={() => changeLanguage('pl')}>pl</button>
        </div>

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

export default withNamespaces()(Home);
