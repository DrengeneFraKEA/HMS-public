import React, { Component } from 'react';
import axios from 'axios';
import Navbar from "./Navbar";
import '../styling/Prescriptions.css';
import '../styling/navbar.css';

export class Prescriptions extends Component {
    constructor(props) {
        super(props);

        this.state = {
            searchText: '',
            searchResults: [],
            prescriptions: [],
            currentCPR: '', // Added to store the current CPR value
            prescriptionStatus: '' // Added to store the prescription status
        };
    }

    componentDidMount() {
        // Check if user is logged in.
        var token = localStorage.getItem("token");
        if (token === "") {
            window.location.href = '/';
            return;
        }
    }

    handleSearchInputChange = (e) => {
        this.setState({
            searchText: e.target.value,
        });
    };

    handleSearchButtonClick = async () => {
        const { searchText } = this.state;

        try {
            const response = await axios.get(`/drug/${searchText}`, {
                headers: { "Authorization": localStorage.getItem('token') }
            });
            const searchResults = response.data;
            this.setState({
                searchResults,
            });
        } catch (error) {
            console.error('Error fetching search results:', error);
        }
    };

    handlePrescribeButtonClick = (drugId) => {
        // Find the drug in searchResults based on its ID
        const { searchResults } = this.state;
        const selectedDrug = searchResults.find(drug => drug.Id === drugId);

        // Log the details of the selected drug
        console.log("User clicked on drug:", selectedDrug);

        // Check if a prescription with CPR input already exists
        const { prescriptions } = this.state;
        const existingPrescriptionIndex = prescriptions.findIndex(prescription => prescription.cprInputOpen);

        // If an existing prescription is found, clear its CPR input field and update the selected drug
        if (existingPrescriptionIndex !== -1) {
            const updatedPrescriptions = [...prescriptions];
            updatedPrescriptions[existingPrescriptionIndex] = {
                ...updatedPrescriptions[existingPrescriptionIndex],
                selectedDrugId: drugId,
                cpr: '', // Clear the CPR input field
            };

            // Update the state with the modified prescription
            this.setState({ prescriptions: updatedPrescriptions });
        } else {
            // Otherwise, add a new prescription
            const dummyPrescription = {
                cprInputOpen: true,
                selectedDrugId: drugId // Store the selected drug ID in state
            };

            // Add the dummy prescription to the state
            this.setState(prevState => ({
                prescriptions: [...prevState.prescriptions, dummyPrescription]
            }));
        }
    };

    handleCPRInputChange = (index, e) => {
        const { prescriptions } = this.state;
        const updatedPrescriptions = [...prescriptions];
        updatedPrescriptions[index].cpr = e.target.value;
        this.setState({
            prescriptions: updatedPrescriptions,
            currentCPR: e.target.value, // Update the current CPR value in state
        });
    };

    SendPrescriptionButtonClicked = async () => {
        const { currentCPR, searchResults } = this.state;
        console.log("CPR from previous prescription:", currentCPR);

        const selectedDrug = searchResults.find(drug => drug.Id === this.state.prescriptions[this.state.prescriptions.length - 1].selectedDrugId);
        console.log("Current selected drug:", selectedDrug);

        try {
            const response = await axios.post("drug/prescribe", {
                cpr: currentCPR,
                drug: selectedDrug
            });

            console.log(response)

            if (response.data === true) {
                this.setState({ prescriptionStatus: 'Recept modtaget!' });
            } else {
                this.setState({ prescriptionStatus: 'Noget gik galt!' });
            }
        } catch (error) {
            console.log("Error:", error);
            this.setState({ prescriptionStatus: 'Error occurred while processing prescription' });
        }
    };

    render() {
        const { searchText, searchResults, prescriptions, prescriptionStatus } = this.state;

        return (
            <>
                <Navbar />
                <div className="prescriptions-container">
                    <div className="search-box">
                        <input
                            type="text"
                            value={searchText}
                            onChange={this.handleSearchInputChange}
                            className="search-input"
                            placeholder="Find stoffer"
                        />
                        <button onClick={this.handleSearchButtonClick} className="search-button">
                            Find
                        </button>
                    </div>

                    <div className="search-results">
                        {searchResults.length > 0 && (
                            <ul className="results-list">
                                {searchResults.map((drug) => (
                                    <li key={drug.Id} className="result-item">
                                        <span className="result-name">{drug.Name}</span>
                                        <button
                                            onClick={() => this.handlePrescribeButtonClick(drug.Id)}
                                            className="prescribe-button">
                                            Skriv recept
                                        </button>
                                    </li>
                                ))}
                            </ul>
                        )}
                    </div>

                    {prescriptions.length > 0 && (
                        <div className="prescriptions-box">
                            <h2>Indtast CPR</h2>
                            <div className="prescriptions-container">
                                {prescriptions.map((prescription, index) => (
                                    <div key={index} className="prescription-item">
                                        <input
                                            type="text"
                                            value={prescription.cpr || ''}
                                            onChange={(e) => this.handleCPRInputChange(index, e)}
                                            className="cpr-input"
                                            placeholder="CPR"
                                        />
                                        <button
                                            onClick={this.SendPrescriptionButtonClicked}
                                            className="kvitter-button">
                                            Kvitter
                                        </button>
                                    </div>
                                ))}
                            </div>
                            {/* Display prescription status */}
                            <div>{prescriptionStatus}</div>
                        </div>
                    )}
                </div>
            </>
        );
    }
}
