import React, { Component } from 'react';
import Navbar from "./Navbar";
import { Link } from 'react-router-dom';
import '../styling/navbar.css';

export class Main extends Component {
    static displayName = Main.name;

    render() {
        return (
            <div>
            <Navbar/>
            </div>
        );
    }
}