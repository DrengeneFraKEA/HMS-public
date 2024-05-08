import React, { Component } from 'react';
import axios from 'axios';
import Navbar from './Navbar';
import '../styling/navbar.css';

export class Rating extends Component {
    constructor(props) {
        super(props)

        this.state = {
            ratings: [],
        };
    }

    componentDidMount() {
        this.fetchRatings();
    }

    fetchRatings = async () => {
        try {
            const response = await axios.get("http://localhost:8080/api/rating");
            this.setState({
                ratings: response.data,
            });
        }
        catch (error) {
            console.error('Error fetching ratings: ', error);
        }
    }

    render() {
        const { ratings } = this.state;

        return (
            <>
                <Navbar />
                <div className="journal-container">
                    <h2>Ratings</h2>
                    <ul>
                        {ratings.map((rating) => (
                            <li key={rating.id}>
                                <div className="journal-note">{rating.title}</div>
                                <div className="journal-date">Text: {rating.text}</div>
                                <div className="journal-date">Score: {rating.score}</div>
                                <div className="journal-date">Date: {rating.modifiedDate}</div>
                                <div className="journal-date">Patient ID: {rating.patientId}</div>
                                <div className="journal-date">Doctor ID: {rating.doctorId}</div>
                            </li>
                        ))}
                    </ul>
                </div>
            </>
        );
    }
}
