import React from 'react';
import './ActivitiesContribution.css';

const ActivitiesContributionItem = (props) => {
  console.log("ActivitiesContributionITEM props: ", props);

  return(
    <li className="ActivitiesContribution-item">
      <div className="ActivitiesContribution-item__category">
        {(props.iconSVG === undefined ? "" : <props.iconSVG className="ActivitiesContribution-item__category__icon" />)}
        {props.item.category}
      </div>
      <div className="ActivitiesContribution-item__progress">
        <div className="ActivitiesContribution-item__progressBar" style={{width: props.progress + '%'}}></div>
      </div>
      <div className="ActivitiesContribution-item__stats">
       {props.item.percent}%
      </div>
    </li>
  )
}

const ActivitiesContribution = (props) =>{
  console.log("ActivitiesContribution props: ", props);

  let topFiveActivitiesContribution = {"topActivities" : [
    {"category": "Run", "percent": 37},
    {"category": "WinterSports", "percent": 22},
    {"category": "Ride", "percent": 21},
    {"category": "Other", "percent": 5},
    {"category": "Fitness", "percent": 1}
  ]}

  const maxPercentContribution = topFiveActivitiesContribution.topActivities[0].percent;
  const progressBar = (percent) =>{
    return(percent * 100 / maxPercentContribution );
  }
  const addIconElement = (category) =>{
    console.log('category', category);
    const categoryDetails = props.categories.find(i => i.category === category);
    if(categoryDetails !== undefined){
      console.log("categoryDetails", categoryDetails.categoryIconComponent);
      return(
        categoryDetails.categoryIconComponent
      );
    }
  }

  return (
    <div className="ActivitiesContribution">
      <p className="ActivitiesContribution-header">Top Activities Contribution</p>
      <ul className="ActivitiesContribution-list">
        {topFiveActivitiesContribution.topActivities.map( item => <ActivitiesContributionItem key={item.category} item={item} progress={progressBar(item.percent)} iconSVG={addIconElement(item.category)} />)}
      </ul>
    </div>
  )
}

export default ActivitiesContribution;
