import React, { Component } from 'react';
import Slider from "react-slick";

import './CharitySlider.css';

class CharitySlider extends Component {
  render() {
    const settings = {
      dots: true,
      infinite: true,
      speed: 500,
      slidesToShow: 1,
      slidesToScroll: 1
    };

    let slides;
    if(this.props.data){
       slides = this.props.data.map(i =>
        <div className="CharitySlider__item" key={i.sys.id}>
          <h4 className="CharitySlider__item-date">{i.fields.monthYear}</h4>
          <h3 className="CharitySlider__item-name">{i.fields.name}</h3>
          <p className="CharitySlider__item-content">{i.fields.description}</p>
        </div>
      );
    }

    return (

      <div className="CharitySlider">
        <div className="CharitySlider__container container">
          <h2 className="CharitySlider__header Header"><strong>... for charity</strong></h2>
          <Slider className="CharitySlider__slider" {...settings}>
            {slides}
          </Slider>
        </div>
      </div>

    );
  }
}

export default CharitySlider;
