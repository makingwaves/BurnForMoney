import React, { Component } from 'react';

import './MonthStats.css';

import medal from 'static/img/medal.svg';
import money from 'static/img/money.svg';
import people from 'static/img/symbol-people.svg';
import { withNamespaces } from 'react-i18next';

const MonthSummaryStatsCard = (props) => {
  console.log('props: ', props)
    return (
        <div className={"MonthSummaryStats-card"}>
            <div className="MonthSummaryStats-card-val">{props.value} {(props.name === 'money' ? <span className="currency">PLN</span> : '')}</div>
            <div className="MonthSummaryStats-card-txt">{props.text}</div>
            <img className="MonthSummaryStats-card-img" src={props.imgSrc} alt="" />
        </div>
    );
}

class MonthSummaryStats extends Component {
    render() {
        const { t } = this.props;
        return (
            <div className="MonthSummaryStats">
                <MonthSummaryStatsCard
                    name={"trainings"}
                    value={(this.props.data ? this.props.data.thisMonth.numberOfTrainings : 0)}
                    text={t("TV_Training_sessions")}
                    imgSrc={medal}
                />

                <MonthSummaryStatsCard
                    name={"engagement"}
                    value={(this.props.data ? this.props.data.thisMonth.percentOfEngagedEmployees : 0) + "%"}
                    text={t("TV_Making_Wavers_engaged")}
                    imgSrc={people}
                />

                <MonthSummaryStatsCard
                    name={"money"}
                    value={(this.props.data ? this.props.data.thisMonth.money : 0)}
                    text={t("TV_collected_so_far")}
                    imgSrc={money}
                />
            </div>
        );
    }
}

export default withNamespaces()(MonthSummaryStats);
