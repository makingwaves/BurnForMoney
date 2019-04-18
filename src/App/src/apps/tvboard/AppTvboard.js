import React, { Component } from 'react';
import logo from 'static/img/logo-white.svg';
import './AppTvboard.css'

import MonthCategoryStats from "./components/MonthStats/MonthCategoryStats";
import MonthSummaryStats from "./components/MonthStats/MonthSummaryStats";

import RankingList from '../../components/Ranking/RankingList/RankingList.js';
import { withNamespaces } from 'react-i18next';

import authFetch from "../../components/Authentication/AuthFetch"

class AppTvboard extends Component {
    internal_api_url = process.env.REACT_APP_DASHBOARD_API_URL;
    public_api_url = process.env.REACT_APP_API_URL;

    monthNames = ["January", "February", "March", "April", "May", "June",
      "July", "August", "September", "October", "November", "December"
  ];

    constructor(props){
        super(props);
        this.state = {
            bfmStats: '',
            lang: localStorage.getItem('language') || 'en',
            ranking: [],
            rankingLoading: true,
            current_month: ''
          };
    }

    render() {
      const { t } = this.props;

        return (
          <div className="Tvboard">
            <div className="Tvboard__layout">
              <div className="Tvboard__layout-head">
                <div className="Tvboard__layout-head_layout">
                  <img className="Tvboard__layout-head-logo" src={logo} alt="Burn for Money" />
                  <div className="Tvboard__layout-head-date" >
                    {this.state.current_month}
                  </div>
                </div>
              </div>
              <div className="Tvboard__layout-board">
                <div className="Tvboard__layout-board__inner-layout">
                  <div className="Tvboard__layout-board__summary">
                    <h4 className="Tvboard__layout-board__header">
                      {t("TV_Summary")}
                    </h4>
                    <div className="Tvboard__layout-board__summary-content">
                      <MonthSummaryStats data={this.state.bfmStats}/>
                    </div>
                  </div>
                  <div className="Tvboard__layout-board__category">
                    <h4 className="Tvboard__layout-board__header">
                      {t("TV_Result_per_category")}
                    </h4>
                    <div className="Tvboard__layout-board__category-content">
                      <MonthCategoryStats data={this.state.bfmStats} /></div>
                    </div>
                  <div className="Tvboard__layout-board__ranking">
                    <h4 className="Tvboard__layout-board__header">
                      {t("TV_TOP_10")}
                    </h4>
                    <div className="Tvboard__layout-board__ranking-content">
                      <RankingList ranking={this.state.ranking} rankingLoading={this.state.rankingLoading} />
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        );
    }

    fetchStats(){
      const { t } = this.props;

      const currentDate = new Date();
      this.setState({current_month: `${t(this.monthNames[currentDate.getMonth()])} ${currentDate.getFullYear()}`});

      authFetch(`${this.public_api_url}/api/totalnumbers`)
        .then(res => res.json())
        .then(
          (result) => {this.setState({ bfmStats: result}); },
          (error) => {this.setState({ bfmStats: null,}); console.error('Error:', error); }
        );

        authFetch(`${this.internal_api_url}/api/ranking?month=${currentDate.getMonth()+1}&year=${currentDate.getFullYear()}`)
        .then(res => res.json())
        .then(
          (result) => {this.setState({ranking: result,  rankingLoading: false });},
          (error) => {this.setState({ranking: null}); console.error('Error:', error); }
        );
        console.log(`FetchData triggered at: ${new Date()}`);
    }

    componentDidMount(){
      this.fetchStats();
      this.fetch_timer = setInterval(() => this.fetchStats(), 15*1000*60);
    }
    componentWillUnmount(){
      clearInterval(this.fetch_timer);
    }
}

export default withNamespaces()(AppTvboard);
