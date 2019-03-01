import React, { Component } from 'react';
import Slider from "react-slick";
import { withNamespaces } from 'react-i18next';


import './CharitySlider.css';

class CharitySlider extends Component {
  render() {
    const { t } = this.props;
    const settings = {
      dots: true,
      infinite: true,
      speed: 500,
      slidesToShow: 1,
      slidesToScroll: 1,
      initialSlide: 1
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
          <h2 className="CharitySlider__header Header"><strong>... {t('for charity')}</strong></h2>
          <Slider className="CharitySlider__slider" ref={c => (this.slider = c)} {...settings}>
            {slides}
          </Slider>
        </div>
      </div>

    );
  }

  componentDidMount() {
    this.slider.slickGoTo(0);
  }
}

export default withNamespaces()(CharitySlider);
