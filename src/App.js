import React from "react";
import AppointmentsAdvanced from "./pages/AppointmentsAdvanced";
import DoctorFormWithUpload from "./pages/DoctorFormWithUpload";
import PrescriptionsList from "./pages/PrescriptionsList";
import PrescriptionForm from "./pages/PrescriptionForm";
import PrescriptionDetails from "./pages/PrescriptionDetails";
import Login from "./pages/Login";
import { Routes, Route } from "react-router-dom";
import DashboardLayout from "./components/DashboardLayout";
import ProtectedRoute from "./components/ProtectedRoute";
import AdminLogin from "./pages/AdminLogin";
import Dashboard from "./pages/Dashboard";
import Doctors from "./pages/Doctors";
import DoctorForm from "./pages/DoctorForm";
import Patients from "./pages/Patients";
import Appointments from "./pages/Appointments";
import Hospitals from "./pages/Hospitals";
import Clinics from "./pages/Clinics";
import NotFound from "./pages/NotFound";
import AddAdmin from "./pages/AddAdmin";
import HospitalFormWithUpload from "./pages/HospitalFormWithUpload";
import ClinicFormWithUpload from "./pages/ClinicFormWithUpload";
import PatientFormWithUpload from "./pages/PatientFormWithUpload";

function App() {
  return (
    <Routes>
      <Route path="/login" element={<AdminLogin />} />
      <Route
        path="/"
        element={
          <ProtectedRoute>
            <DashboardLayout />
          </ProtectedRoute>
        }
      >
        <Route path="appointments" element={<AppointmentsAdvanced />} />
        <Route path="doctors/new" element={<DoctorFormWithUpload />} />
        <Route path="doctors/:id" element={<DoctorFormWithUpload />} />
        <Route path="hospitals/new" element={<HospitalFormWithUpload />} />
        <Route path="hospitals/:id" element={<HospitalFormWithUpload />} />
        <Route path="clinics/new" element={<ClinicFormWithUpload />} />
        <Route path="clinics/:id" element={<ClinicFormWithUpload />} />
        <Route path="patient/new" element={<PatientFormWithUpload />} />
        <Route path="patient/:id" element={<PatientFormWithUpload />} />
        <Route path="prescriptions" element={<PrescriptionsList />} />
        <Route path="prescriptions/new" element={<PrescriptionForm />} />
        <Route path="prescriptions/edit/:id" element={<PrescriptionForm />} />
        <Route path="prescriptions/:id" element={<PrescriptionDetails />} />
        <Route index element={<Dashboard />} />
        <Route path="dashboard" element={<Dashboard />} />
        <Route path="doctors" element={<Doctors />} />
        <Route path="doctors/new" element={<DoctorForm />} />
        <Route path="doctors/:id" element={<DoctorForm />} />
        <Route path="patients" element={<Patients />} />
        <Route path="appointments" element={<Appointments />} />
        <Route path="hospitals" element={<Hospitals />} />
        <Route path="clinics" element={<Clinics />} />

        <Route
          path="prescriptions"
          element={<div>Prescriptions - To implement</div>}
        />
        <Route
          path="admins/new"
          element={
            <ProtectedRoute role="Admin">
              <AddAdmin />
            </ProtectedRoute>
          }
        />
        <Route path="*" element={<NotFound />} />
      </Route>
    </Routes>
  );
}

export default App;
