import React, { Component } from 'react';
import Slider from "react-slick";

import './CharitySlider.css';

class CharitySlider extends Component {
  render() {
    var settings = {
      dots: true,
      infinite: true,
      speed: 500,
      slidesToShow: 1,
      slidesToScroll: 1
    };

    return (
      <div className="CharitySlider">
        <div className="CharitySlider__container container">
          <h2 className="CharitySlider__header Header"><strong>... for charity</strong></h2>
          <Slider className="CharitySlider__slider" {...settings}>
          <div className="CharitySlider__item">
            <h4 className="CharitySlider__item-date">July 2018</h4>
            <h3 className="CharitySlider__item-name">Nazwa fundacji/inicjatywy</h3>
            <p className="CharitySlider__item-content">Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo.</p>
          </div>
          <div className="CharitySlider__item">
            <h4 className="CharitySlider__item-date">June 2018</h4>
            <h3 className="CharitySlider__item-name">Nazwa fundacji </h3>
            <p className="CharitySlider__item-content">Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo.</p>
          </div>
          <div className="CharitySlider__item">
            <h4 className="CharitySlider__item-date">May 2018</h4>
            <h3 className="CharitySlider__item-name">Nazwa inicjatywy</h3>
            <p className="CharitySlider__item-content">Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo.</p>
          </div>
          <div className="CharitySlider__item">
            <h4 className="CharitySlider__item-date">April 2018</h4>
            <h3 className="CharitySlider__item-name">Lelum Polelum</h3>
            <p className="CharitySlider__item-content">Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo.</p>
          </div>
          </Slider>
        </div>
      </div>

    );
  }
}

export default CharitySlider;
